using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class LtiLaunchConfiguration : EntityTypeConfiguration<LtiLaunch>
    {
        public LtiLaunchConfiguration() : this("reports") { }

        public LtiLaunchConfiguration(string schema)
        {
            ToTable(schema + ".LtiLaunch");
            HasKey(x => new { x.LaunchKey });
            Property(x => x.AssetNumber).IsOptional();
            Property(x => x.context_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.context_label).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.context_title).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.context_type).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_api_domain).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_assignment_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_assignment_points_possible).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_assignment_title).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_course_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_enrollment_state).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_user_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_user_login_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_canvas_workflow_state).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_context_memberships_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_context_setting_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_genericmediaurl).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_lineitem_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_lineitems_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_link_memberships_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_link_setting_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_lp).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_result_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_results_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_system_setting_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.custom_tc_profile_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lis_basic_outcome_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lis_memberships_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lis_memberships_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lis_resultvalue_sourcedids).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lti_tool_setting_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_ims_lti_tool_setting_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_lti_assignment_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_outcome_data_values_accepted).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_outcome_result_total_score_accepted).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_outcome_submission_submitted_at_accepted).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_outcomes_tool_placement_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ext_roles).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.launch_presentation_css_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.launch_presentation_document_target).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.launch_presentation_locale).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.launch_presentation_return_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LaunchKey).IsRequired();
            Property(x => x.LaunchParameters).IsRequired().IsUnicode(false);
            Property(x => x.LaunchSignature).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.LaunchTime).IsRequired();
            Property(x => x.LaunchType).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.LaunchUrl).IsRequired().IsUnicode(false).HasMaxLength(300);
            Property(x => x.lis_course_offering_sourcedid).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_course_section_sourcedid).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_outcome_service_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_person_contact_email_primary).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_person_name_family).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_person_name_full).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_person_name_given).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_person_sourcedid).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lis_result_sourcedid).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lti_message_type).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.lti_version).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_callback).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_consumer_key).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_nonce).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_signature).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_signature_method).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_timestamp).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.oauth_version).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.resource_link_description).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.resource_link_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.resource_link_title).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.roles).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.TenantCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.tool_consumer_info_product_family_code).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_info_version).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_instance_contact_email).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_instance_description).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_instance_guid).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_instance_name).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.tool_consumer_instance_url).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.user_id).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.user_image).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ValidationErrors).IsOptional().IsUnicode(false);
        }
    }
}
